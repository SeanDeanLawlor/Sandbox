from flask import Flask, request, jsonify
from zx_llm_bridge import rewrite_with_llm

app = Flask(__name__)

@app.route('/rewrite', methods=['POST'])
def rewrite():
    data = request.get_json()
    circuit = data.get("circuit", "")

    result = rewrite_with_llm(circuit)

    return jsonify({
        "original": circuit,
        "rewritten": result["rewritten"],
        "rules_applied": result["rules_applied"]
    })

if __name__ == '__main__':
    app.run(port=5050)