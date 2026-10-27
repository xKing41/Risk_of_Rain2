"""Erzeugt für jedes Item in MoreItems/Items/ItemList.cs ein Icon (Symbol + Seltenheitsfarbe)."""
import re
import subprocess
import sys
from PIL import Image, ImageDraw, ImageFont

COLORS = {"T1": "d9d9d9", "T2": "6ee87a", "T3": "e85a5a", "Boss": "e8d34f", "Lunar": "4fa3e8"}
src = open("MoreItems/Items/ItemList.cs", encoding="utf-8").read()
items = re.findall(r'\b(?:Stat|StatB|Hit|Kill|Hurt|Cond|Pulse)\("(\w+)", "[^"]*", (T1|T2|T3|Boss|Lunar), "([^"]+)"', src)
font = ImageFont.truetype("DejaVuSans-Bold.ttf", 64)


def render(text):
    img = Image.new("L", (96, 96))
    ImageDraw.Draw(img).text((48, 48), text, font=font, fill=255, anchor="mm")
    return img.tobytes()


tofu = render(chr(0xFFFF))
missing = [f"{name} ({sym})" for name, _, sym in items if render(sym) == tofu]
if missing:
    sys.exit("Symbole fehlen in der Schrift: " + ", ".join(missing))

specs = [f"{name}:{sym}:{COLORS[tier]}" for name, tier, sym in items]
subprocess.check_call([sys.executable, "tools/make_icons.py", "MoreItems/Icons", *specs])
print(len(specs), "Item-Icons erzeugt")
