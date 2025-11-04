import ollama
from flask import Flask, jsonify, request
from flask_cors import CORS

app = Flask(__name__)
# Enable CORS for Electron app
CORS(app) 

@app.route('/api/health', methods=['GET'])
def health_check():
    return jsonify({"status": "ok", "message": "Server is running"})

@app.route('/api/talk_to_llm', methods=['POST'])
def talk_to_llm():
    data = request.get_json()
    print(data)
    response = ollama.chat(
        model = 'llm_language', 
        messages=
        [
            {'role': 'user', 'content': data},
        ],)
    return response['message']['content']

if __name__ == '__main__':
    app.run(debug=True, port=5000)