#!/usr/bin/env python3
"""
Generates a separate tileset image with water color RGB(20, 50, 150) drawn behind
the transparent parts of the sand-to-water transition tiles (columns 3..5, rows 0..4).
"""

from PIL import Image
import os

def generate_water_tileset():
    base_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    input_path = os.path.join(base_dir, 'assets', 'tileset.png')
    output_path = os.path.join(base_dir, 'assets', 'tileset_water.png')

    if not os.path.exists(input_path):
        raise FileNotFoundError(f"Source tileset not found at {input_path}")

    im = Image.open(input_path).convert('RGBA')
    im_water = im.copy()
    bg_color = (40, 100, 200, 255)

    # Composite water color behind sand-to-water transition tiles (columns 3..5, rows 0..9)
    for r in range(0, 10):
        for c in range(3, 6):
            box = (c * 16, r * 16, (c + 1) * 16, (r + 1) * 16)
            tile = im.crop(box)
            bg = Image.new('RGBA', (16, 16), bg_color)
            comp = Image.alpha_composite(bg, tile)
            im_water.paste(comp, box)

    im_water.save(output_path)
    print(f"Saved modified tileset with water background to: {output_path}")

if __name__ == '__main__':
    generate_water_tileset()
