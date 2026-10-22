import os
import sys
import wave

import numpy as np

SR = 44100
OUT_DIR = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "Assets", "Resources", "Audio", "SFX")


def timeline(duration):
    return np.arange(int(SR * duration)) / SR


def silence(duration):
    return np.zeros(int(SR * duration))


def place(buffer, clip, start):
    i = int(SR * start)
    end = min(len(buffer), i + len(clip))
    buffer[i:end] += clip[: end - i]
    return buffer


def fade(signal, fade_in=0.004, fade_out=0.02):
    n_in = max(1, int(SR * fade_in))
    n_out = max(1, int(SR * fade_out))
    signal[:n_in] *= np.linspace(0, 1, n_in)
    signal[-n_out:] *= np.linspace(1, 0, n_out)
    return signal


def normalize(signal, peak=0.89):
    m = np.max(np.abs(signal))
    return signal if m == 0 else signal * (peak / m)


def echo(signal, delays=((0.09, 0.3), (0.18, 0.15)), tail=0.25):
    out = np.concatenate([signal, silence(tail)])
    for delay, gain in delays:
        place(out, signal * gain, delay)
    return out


def lowpass(signal, cutoff):
    alpha = np.exp(-2 * np.pi * cutoff / SR)
    out = np.empty_like(signal)
    y = 0.0
    for i, x in enumerate(signal):
        y = (1 - alpha) * x + alpha * y
        out[i] = y
    return out


def bandpass_sweep(signal, centers, q):
    out = np.empty_like(signal)
    low = band = 0.0
    damp = 1.0 / q
    for i, x in enumerate(signal):
        f = 2 * np.sin(np.pi * min(centers[i], SR / 6) / SR)
        high = x - low - damp * band
        band += f * high
        low += f * band
        out[i] = band
    return out


def soft_square(freq, t, harmonics=7):
    wave_sum = np.zeros_like(t)
    for n in range(1, harmonics * 2, 2):
        wave_sum += np.sin(2 * np.pi * freq * n * t) / n
    return wave_sum


def bell(freq, duration, decay):
    t = timeline(duration)
    tone = (np.sin(2 * np.pi * freq * t)
            + 0.35 * np.sin(2 * np.pi * freq * 2.0 * t) * np.exp(-t / (decay * 0.5))
            + 0.12 * np.sin(2 * np.pi * freq * 3.01 * t) * np.exp(-t / (decay * 0.3)))
    attack = np.minimum(1, t / 0.005)
    return tone * attack * np.exp(-t / decay)


def boomerang_throw(seed, peak_freq, spin_rate):
    rng = np.random.default_rng(seed)
    duration = 0.5
    t = timeline(duration)
    p = t / duration
    noise = rng.uniform(-1, 1, len(t))
    centers = 350 + (peak_freq - 350) * np.sin(np.pi * p) ** 1.5
    whoosh = bandpass_sweep(noise, centers, q=2.5)
    envelope = np.sin(np.pi * p) ** 2
    spin = 1 - 0.45 * (0.5 + 0.5 * np.sin(2 * np.pi * spin_rate * t))
    return fade(normalize(whoosh * envelope * spin, 0.8), 0.01, 0.05)


def boomerang_hit(seed, body_freq, knock_freq):
    rng = np.random.default_rng(seed)
    duration = 0.35
    t = timeline(duration)
    body_pitch = 60 + (body_freq - 60) * np.exp(-t / 0.04)
    body_phase = 2 * np.pi * np.cumsum(body_pitch) / SR
    body = np.sin(body_phase) * np.exp(-t / 0.09)
    knock = np.sin(2 * np.pi * knock_freq * t) * np.exp(-t / 0.025)
    click = np.diff(rng.uniform(-1, 1, len(t) + 1)) * np.exp(-t / 0.006)
    crunch = lowpass(rng.uniform(-1, 1, len(t)), 2500) * np.exp(-t / 0.03)
    mix = 1.0 * body + 0.55 * knock + 0.35 * click + 0.6 * crunch
    return normalize(fade(mix, 0.001, 0.03))


def invalid_selection():
    out = silence(0.36)
    for start, freq, length in ((0.0, 196.0, 0.13), (0.16, 155.6, 0.18)):
        t = timeline(length)
        envelope = np.minimum(1, t / 0.008) * np.minimum(1, (length - t) / 0.03)
        tone = soft_square(freq, t, harmonics=6) * envelope
        place(out, tone, start)
    out = lowpass(out, 1800)
    return fade(normalize(out, 0.75), 0.002, 0.02)


def arpeggio(freqs, step, decay, shimmer_seed):
    out = silence(step * len(freqs) + decay * 4)
    for i, f in enumerate(freqs):
        place(out, bell(f, decay * 4, decay) * (1 - 0.08 * i), step * i)
    rng = np.random.default_rng(shimmer_seed)
    t = timeline(len(out) / SR)
    shimmer = lowpass(rng.uniform(-1, 1, len(t)), 6000) - lowpass(rng.uniform(-1, 1, len(t)), 3000)
    shimmer *= np.exp(-t / (decay * 1.5)) * 0.06
    out = echo(out + shimmer, delays=((0.11, 0.28), (0.22, 0.12)), tail=0.3)
    return fade(normalize(out), 0.002, 0.08)


def platform_enter():
    return arpeggio([523.25, 659.25, 783.99, 1046.50], step=0.075, decay=0.22, shimmer_seed=11)


def platform_exit():
    return arpeggio([1046.50, 783.99, 659.25, 523.25], step=0.075, decay=0.22, shimmer_seed=12)


def coin_collect():
    out = silence(0.42)
    first = timeline(0.075)
    place(out, soft_square(987.77, first, harmonics=3) * np.minimum(1, first / 0.003), 0.0)
    second = timeline(0.34)
    place(out, soft_square(1318.51, second, harmonics=3) * np.exp(-second / 0.12), 0.075)
    out = lowpass(out, 7000)
    return fade(normalize(out, 0.8), 0.002, 0.03)


def write_wav(path, signal):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    data = (np.clip(signal, -1, 1) * 32767).astype("<i2")
    with wave.open(path, "wb") as f:
        f.setnchannels(1)
        f.setsampwidth(2)
        f.setframerate(SR)
        f.writeframes(data.tobytes())


SOUNDS = {
    "BoomerangThrow": [
        ("throw_01", lambda: boomerang_throw(1, 2400, 16)),
        ("throw_02", lambda: boomerang_throw(2, 2000, 19)),
    ],
    "BoomerangHit": [
        ("hit_01", lambda: boomerang_hit(3, 180, 520)),
        ("hit_02", lambda: boomerang_hit(4, 150, 610)),
        ("hit_03", lambda: boomerang_hit(5, 210, 450)),
    ],
    "InvalidSelection": [("error_01", invalid_selection)],
    "PlatformEnter": [("platform_enter_01", platform_enter)],
    "PlatformExit": [("platform_exit_01", platform_exit)],
    "CoinCollect": [("coin_01", coin_collect)],
}


def main():
    out_dir = sys.argv[1] if len(sys.argv) > 1 else OUT_DIR
    for folder, clips in SOUNDS.items():
        for name, build in clips:
            signal = build()
            path = os.path.join(out_dir, folder, f"{name}.wav")
            write_wav(path, signal)
            print(f"{folder}/{name}.wav  {len(signal) / SR:.2f}s  peak {np.max(np.abs(signal)):.2f}")


if __name__ == "__main__":
    main()
