import ollama

response = ollama.chat(
    model='llm_language',  # Specify the model you want to use
    messages=[
        {'role': 'user', 'content': '{"new_page":{"input_language":"English","page_category":"Gaming"}}'},
    ],
)
print(response['message']['content'])