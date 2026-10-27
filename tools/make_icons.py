"""Erzeugt Skill-/Item-Icons aus einer Liste: python3 tools/make_icons.py <Ausgabeordner> <Name>:<Symbol>:<Farbe-hex> ...

Bis wir echte Artwork-Icons haben, bekommt jedes Skill und Item ein eigenes Symbol-Icon,
damit man sie im Spiel unterscheiden kann.
"""
import os
import sys
from PIL import Image, ImageDraw, ImageFilter, ImageFont

SIZE = 128
FONT = ImageFont.truetype("DejaVuSans-Bold.ttf", 64)


def icon(symbol, color):
    c = tuple(int(color[i:i + 2], 16) for i in (0, 2, 4))
    img = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    for i in range(SIZE // 2):
        t = i / (SIZE / 2)
        fill = tuple(int(v * (0.15 + 0.45 * t)) for v in c) + (255,)
        d.rounded_rectangle((i, i, SIZE - 1 - i, SIZE - 1 - i), radius=max(2, 22 - i), fill=fill)
    d.rounded_rectangle((3, 3, SIZE - 4, SIZE - 4), radius=20, outline=c + (255,), width=5)

    glow = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    ImageDraw.Draw(glow).text((SIZE / 2, SIZE / 2), symbol, font=FONT, fill=c + (255,), anchor="mm")
    img.alpha_composite(glow.filter(ImageFilter.GaussianBlur(6)))
    ImageDraw.Draw(img).text((SIZE / 2, SIZE / 2), symbol, font=FONT, fill=(255, 255, 255, 255), anchor="mm")
    return img


out = sys.argv[1]
os.makedirs(out, exist_ok=True)
for spec in sys.argv[2:]:
    name, symbol, color = spec.split(":")
    icon(symbol, color).save(os.path.join(out, name + ".png"))
