# zx_llm_bridge.py (Multi-Provider LLM Support)
import os
import openai
import requests
from flask import Flask, request, jsonify

app = Flask(__name__)

# === Load env and settings ===
from dotenv import load_dotenv
load_dotenv()

PROVIDER = os.getenv("LLM_PROVIDER", "openai")
ENDPOINT = os.getenv("LLM_ENDPOINT", "http://localhost:5050/rewrite")
openai.api_key = os.getenv("OPENAI_API_KEY")

@app.route('/rewrite', methods=['POST'])
def rewrite():
    data = request.json
    circuit = data.get('circuit', '')
    mode = data.get('mode', 'rewrite')

    try:
        if PROVIDER == "openai":
            return jsonify(use_openai(circuit, mode))
        elif PROVIDER == "custom":
            return jsonify(forward_to_custom(circuit, mode))
        elif PROVIDER == "local":
            return jsonify(use_local_model(circuit, mode))
        else:
            return jsonify({
                "original": circuit,
                "rewritten": circuit,
                "rules_applied": ["Unsupported provider"],
                "reasoning": "Please set a supported LLM_PROVIDER."
            })
    except Exception as e:
        return jsonify({
            "original": circuit,
            "rewritten": circuit,
            "rules_applied": ["Error: " + str(e)],
            "reasoning": "Internal server error."
        })

def use_openai(circuit, mode):
    prompt = f"""You are a quantum circuit simplifier using ZX-calculus.

Mode: {mode}
Circuit:
{circuit}

Return a simplified circuit, the rewrite rules, and brief reasoning."""

    response = openai.ChatCompletion.create(
        model="gpt-4",
        messages=[
            {"role": "system", "content": "You simplify circuits using ZX-calculus."},
            {"role": "user", "content": prompt}
        ]
    )

    content = response.choices[0].message.content.strip()
    lines = content.splitlines()
    rewritten = lines[0] if lines else circuit
    rules = lines[1:] if len(lines) > 1 else ["No rules"]

    return {
        "original": circuit,
        "rewritten": rewritten,
        "rules_applied": rules,
        "reasoning": "Simplified using OpenAI GPT-4"
    }

def use_local_model(circuit, mode):
    return {
        "original": circuit,
        "rewritten": circuit.replace("T", ""),
        "rules_applied": ["Removed T gates", "Local model ZX simplification"],
        "reasoning": "Simulated local logic (placeholder)"
    }

def forward_to_custom(circuit, mode):
    payload = {"circuit": circuit, "mode": mode}
    res = requests.post(ENDPOINT, json=payload)
    return res.json()

if __name__ == '__main__':
    app.run(debug=True, port=5050)
