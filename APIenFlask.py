from flask import Flask, request, jsonify
from flask_sqlalchemy import SQLAlchemy
from flask_cors import CORS

app = Flask(__name__)
CORS(app)

app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///positions.db'
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
db = SQLAlchemy(app)

class PlayerState(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    x = db.Column(db.Float, nullable=False)
    y = db.Column(db.Float, nullable=False)
    z = db.Column(db.Float, nullable=False)
    mood = db.Column(db.String(10), nullable=False)

with app.app_context():
    db.create_all()

@app.route('/player/position', methods=['POST'])
def receive_position():
    data = request.get_json()
    if not data:
        return jsonify({"error": "No data received"}), 400

    x = data.get("x")
    y = data.get("y")
    z = data.get("z")
    mood = data.get("mood")

    if mood not in ['happy', 'sad']:
        return jsonify({"error": "Mood must be 'happy' or 'sad'"}), 400

    new_state = PlayerState(x=x, y=y, z=z, mood=mood)
    print(f"Received position: x={x}, y={y}, z={z}, mood={mood}")
    db.session.add(new_state)
    db.session.commit()

    return jsonify({"message": "Position saved", "id": new_state.id}), 201

@app.route('/player/positions', methods=['GET'])
def get_positions():
    all_positions = PlayerState.query.all()
    return jsonify([
        {"id": p.id, "x": p.x, "y": p.y, "z": p.z, "mood": p.mood}
        for p in all_positions
    ])
    
@app.route('/player/last_position', methods=['GET'])
def get_last_position():
    last = PlayerState.query.order_by(PlayerState.id.desc()).first()
    if not last:
        return jsonify({"error": "No data found"}), 404

    return jsonify({
        "id": last.id,
        "x": last.x,
        "y": last.y,
        "z": last.z,
        "mood": last.mood
    })

@app.route('/player/clear_checkpoints', methods=['DELETE'])
def clear_checkpoints():
    secret = request.args.get("secret")
    if secret != "1234": 
        return jsonify({"error": "Unauthorized"}), 401

    deleted = db.session.query(PlayerState).delete()
    print(f"Deleted {deleted} checkpoints")
    db.session.commit()
    return jsonify({"message": f"{deleted} checkpoints deleted"}), 200

@app.route('/player/last_mood', methods=['GET'])
def get_last_mood():
    last = PlayerState.query.order_by(PlayerState.id.desc()).first()
    if not last:
        return jsonify({"error": "No mood found"}), 404

    return jsonify({"mood": last.mood}), 200

if __name__ == '__main__':
    app.run(debug=True)
