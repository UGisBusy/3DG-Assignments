from PIL import Image, ImageDraw

ICON_DIR = "Assets/Art/MinimapIcons"
S = 128  # icon size in pixels

def make_icon(name, draw_fn):
    img = Image.new("RGBA", (S, S), (0, 0, 0, 0))  # transparent background
    draw_fn(ImageDraw.Draw(img))
    img.save(f"{ICON_DIR}/{name}.png")

# Target: white circle with a dark outline
make_icon("IconTarget", lambda d: (
    d.ellipse([4, 4, S - 5, S - 5], fill=(40, 40, 40, 255)),       # outer dark circle
    d.ellipse([16, 16, S - 17, S - 17], fill=(255, 255, 255, 255)), # inner white circle
))

# Obstacle: white square with a dark outline
make_icon("IconObstacle", lambda d: (
    d.rectangle([8, 8, S - 9, S - 9], fill=(40, 40, 40, 255)),
    d.rectangle([20, 20, S - 21, S - 21], fill=(255, 255, 255, 255)),
))

# Player: arrow pointing up (up in the image = forward once the quad lies flat)
make_icon("IconPlayer", lambda d: (
    d.polygon([(64, 2), (122, 124), (64, 92), (6, 124)], fill=(40, 40, 40, 255)),
    d.polygon([(64, 22), (106, 110), (64, 84), (22, 110)], fill=(255, 255, 255, 255)),
))

