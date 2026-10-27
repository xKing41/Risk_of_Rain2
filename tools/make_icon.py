"""Erzeugt ein 256x256 Thunderstore-Icon: python3 tools/make_icon.py <Ausgabe.png> <Text> <Farbe-hex>"""
import sys
from PIL import Image, ImageDraw, ImageFont

out, text, color = sys.argv[1], sys.argv[2], sys.argv[3]
size = 256
img = Image.new("RGBA", (size, size), (20, 22, 30, 255))
d = ImageDraw.Draw(img)
c = tuple(int(color[i:i + 2], 16) for i in (0, 2, 4))
d.ellipse((16, 16, size - 16, size - 16), outline=c, width=12)
d.ellipse((44, 44, size - 44, size - 44), fill=tuple(v // 3 for v in c))
font = ImageFont.truetype("DejaVuSans-Bold.ttf", 72 if len(text) <= 2 else 48)
d.text((size / 2, size / 2), text, font=font, fill=c, anchor="mm")
img.save(out)
