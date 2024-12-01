import sys
from diffusers import StableDiffusionPipeline, DDIMScheduler
import torch
from datetime import datetime

prompt=sys.argv[1]

model_name = "SfinOe/stable-diffusion-v1.5"
local_directory = "C:/Users/walke/Unity Projects/AI-generated Visual Novel/Assets/Models/Image model"

pipeline = StableDiffusionPipeline.from_pretrained(local_directory, torch_dtype=torch.float16)
pipeline.scheduler = DDIMScheduler.from_config(pipeline.scheduler.config)
pipeline.enable_attention_slicing()
device="cuda" if torch.cuda.is_available() else "cpu"
pipeline.to(device)

image = pipeline(prompt, num_inference_steps=20, guidance_scale=7.5).images[0]
timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
imageLocation = f"C:/Users/walke/Unity Projects/AI-generated Visual Novel/Assets/Models/Image model/Generated Images/generated_image_{timestamp}.png"
image.save(imageLocation)
# Save the image locally

