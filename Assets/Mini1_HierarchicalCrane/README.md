# Mini-exercise 4.A.1 – Animation of a hierarchical model

A simple crane holding a spinning saw blade, built by hand in the editor.

## Components

| Component | What it does | Attach to |
| --- | --- | --- |
| `CraneJoint` | Rotates the object about one local axis. **Mode** dropdown: *Manual* (hold CW / CCW key), *AutoCW* (keeps rotating clockwise), *AutoCCW* (keeps rotating counter-clockwise), *Swing* (back and forth between min and max). Keys are ignored in auto modes. | A_Base, B_Arm, C_Blade |
| `PeriodicSlider` | Moves the object along its parent's axis: `rest + axis * A * sin(2*pi*f*t)` | C_Blade |

Both keep the rotation / position you set in the editor as the rest pose.
Select an object to see its axis in the Scene view (yellow = rotation axis, cyan = slide range).

## Hierarchy to build

Use **empty GameObjects (scale 1) as the moving parts**, and put the scaled shapes under them.
Otherwise the scale of a parent stretches its children.

```
Ground                 Plane
A_Base                 empty, (0,0,0)            + CraneJoint  axis (0,1,0), keys A / D
├── HalfSphere         Sphere, scale 3           (centred on the ground, so only the top half shows)
└── B_Arm              empty, (0,1.5,0)          + CraneJoint  axis (1,0,0), keys S / W, limits -20..10
    ├── ArmBox         Cube, scale (0.4,0.4,5), pos (0,1.25,2.17), rot (-30,0,0)
    └── C_Blade        empty, pos (0,0.9,4.33)   + PeriodicSlider  axis (0,1,0)
                                                 + CraneJoint      axis (1,0,0), keys E / Q, speed 360
        ├── Disk       Cylinder, scale (2,0.05,2), rot (0,0,90)
        └── Tooth x4   Cube, scale (0.1,0.5,0.3), pos (0,1,0)/(0,0,1)/(0,-1,0)/(0,0,-1),
                       rot (0,0,0)/(90,0,0)/(180,0,0)/(270,0,0)
```

Requirement mapping:
- A rotates about its y-axis, B about its x-axis (different axes).
- C is a child of B, so `PeriodicSlider` moves it along **B's y-axis** with a sine.
- In Manual mode, the CW / CCW keys change each joint's rotation direction (keys above are only suggestions).
