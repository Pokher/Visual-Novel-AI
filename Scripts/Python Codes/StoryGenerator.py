import sys
from transformers import GPT2Tokenizer, GPT2LMHeadModel
import torch

prompt=sys.argv[1]
length = int(sys.argv[2])

model_name = "t5-large"
local_directory = "C:/Users/walke/Unity Projects/AI-generated Visual Novel/Assets/Models/Story model 2"
story_directory = "C:/Users/walke/Unity Projects/AI-generated Visual Novel/Assets/Models/Generated Stories/story2.txt"

device="cuda" if torch.cuda.is_available() else "cpu"


tokenizer = GPT2Tokenizer.from_pretrained(local_directory)
model = GPT2LMHeadModel.from_pretrained(local_directory).to(device)

input_ids = tokenizer(prompt, return_tensors="pt").input_ids.to(device)
output = model.generate(input_ids, max_length=length, num_return_sequences=1, temperature=0.8)
generated_story = tokenizer.decode(output[0], skip_special_tokens=True)

with open(story_directory, "w", encoding="utf-8") as story_file:
    story_file.write(generated_story)
