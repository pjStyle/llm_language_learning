import ollama
from flask import Flask, jsonify, request
from flask_cors import CORS

app = Flask(__name__)
# Enable CORS for Electron app
CORS(app) 

@app.route('/api/health', methods=['GET'])
def health_check():
    return jsonify({"status": "ok", "message": "Server is running"})

@app.route('/api/talk_to_llm', methods=['GET'])
def talk_to_llm():
    json_request = request.args.get('json', '')
    print("talk_to_llm")
    if (json_request != ''):
        response = ollama.chat(
            model = 'llm_language', 
            messages=
            [
                {'role': 'user', 'content': json_request},
            ],)
        response_to_send = response['message']['content']
        print(response_to_send)
        return response_to_send
    
    return jsonify({"error": "Invalid request"}), 400

if __name__ == '__main__':
    app.run(debug=True, port=5000)