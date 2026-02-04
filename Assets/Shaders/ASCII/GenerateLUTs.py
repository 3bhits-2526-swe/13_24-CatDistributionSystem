from PIL import Image, ImageDraw, ImageFont
import os

script_dir = os.path.dirname(os.path.abspath(__file__))
output_path = os.path.join(script_dir, "ascii_pngs")


def generate_text_images(text_list, output_folder=output_path):
    if not os.path.exists(output_folder):
        os.makedirs(output_folder)

    # We use a built-in PIL font but force it to behave.
    # If you have a specific .ttf pixel font, replace "arial.ttf" with it.
    try:
        # 8pt is usually the magic number for 8px height
        font = ImageFont.truetype("arial.ttf", 8) 
    except:
        font = ImageFont.load_default()

    for i, text in enumerate(text_list):
        # 1. Create a grayscale image (8-bit) - usually cleaner than 1-bit
        img = Image.new('L', (80, 8), color=0) 
        draw = ImageDraw.Draw(img)

        # 2. Disable font smoothing (aliasing) to keep it "pixel-perfect"
        # Note: TrueType fonts usually smooth by default; 
        # for 8px, we want the raw pixels.
        draw.text((0, -1), text, font=font, fill=255)

        # 3. Save with ZERO extra data
        filepath = os.path.join(output_folder, f"{i}.png")
        
        # We use 'pnginfo' to ensure no timestamps or software tags are saved
        img.save(filepath, "PNG", optimize=True, pnginfo=None)
        
        size = os.path.getsize(filepath)
        print(f"Text: {text} | Size: {size} bytes")


my_strings = [".:-=+*#%█"]
generate_text_images(my_strings)
