# Program adapted from ChatGPT response: https://chatgpt.com/share/6720f1b3-23a0-8005-833e-b4d84e186d76

from flask import Flask, request, jsonify
from geopy.distance import geodesic
import time

app = Flask(__name__)

# Dictionary to store player data
players_data = {}

# Proximity radius in meters
PROXIMITY_RADIUS = 10
# Update frequency in seconds
UPDATE_FREQUENCY = 100

@app.route('/generate_player_id', methods=['POST'])
def generate_player_id():
    data = request.get_json()
    name = data.get("Name")
    avatar = data.get("Avatar")
    location = data.get("location")

    player_id = len(players_data)
    players_data[player_id] = {
        "Name": name,
        "Avatar": avatar,
        "location": location,
        "timestamp": time.time()
    }

    print(players_data)
    
    return jsonify({"player_id": player_id}), 200

@app.route('/get_nearby_players', methods=['POST'])
def get_nearby_players():
    data = request.get_json()
    player_id = data.get("player_id")
    latitude = data.get("latitude")
    longitude = data.get("longitude")

    if player_id is None or latitude is None or longitude is None:
        return jsonify({"error": "Invalid data"}), 400

    # Update player location on server
    players_data[player_id]["location"] = {
        "latitude": latitude,
        "longitude": longitude
    }
    current_timestamp = time.time()
    players_data[player_id]["timestamp"] = current_timestamp

    # Get nearby players
    nearby_players = {}
    current_position = (latitude, longitude)

    for other_id, info in players_data.items():
        if other_id == player_id:
            continue

        # Get the other player's name
        name = info["Name"]

        # Get the other player's avatar
        avatar = info["Avatar"]
        
        # Calculate the distance
        other_position = (info["location"]["latitude"], info["location"]["longitude"])
        distance = geodesic(current_position, other_position).meters

        # Calculate update timing difference between players
        other_timestamp = info["timestamp"]
        time_difference = current_timestamp - other_timestamp

        if distance <= PROXIMITY_RADIUS and time_difference <= UPDATE_FREQUENCY:
            nearby_players[other_id] = {
                "Name": name,
                "Avatar": avatar
            }
            print(players_data[player_id]["Name"] + " passed by " + name)

    return jsonify(nearby_players), 200

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
