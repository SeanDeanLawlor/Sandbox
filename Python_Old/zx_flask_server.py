from flask import Flask, request, jsonify
import json

app = Flask(__name__)

@app.route('/rewrite', methods=['POST'])
def rewrite():
    data = request.get_json()
    circuit = data.get("circuit", "")

    # Placeholder rewrite logic
    rewritten = circuit.replace("CNOT", "Z") if "CNOT" in circuit else circuit
    rules = ["CNOT → Z simplification"] if "CNOT" in circuit else ["No simplification applied"]

    return jsonify({
        "original": circuit,
        "rewritten": rewritten,
        "rules_applied": rules
    })

if __name__ == '__main__':
    app.run(port=5050)