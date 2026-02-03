#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include <DHT.h>

#define DHTPIN 4
#define DHTTYPE DHT11

DHT dht(DHTPIN, DHTTYPE);
// WiFi credentials
const char* ssid = "x";
const char* password = "x";

const char* serverUrl = "http://x:5101/api/Node/upload";

void setup() {
  Serial.begin(115200);

  analogReadResolution(12);
  dht.begin();
  // Connect to WiFi
  WiFi.begin(ssid, password);
  Serial.print("Connecting to WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nConnected!");
}

void loop() {
  float temperature = dht.readTemperature();
  float humidity = dht.readHumidity();

  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;

    http.begin(serverUrl);
    http.addHeader("Content-Type", "application/json");

    JsonDocument doc;
    doc["NodeName"] = "ESP32_1";

    JsonArray sensors = doc.createNestedArray("Sensors");

    JsonObject sensor1 = sensors.add<JsonObject>();
    sensor1["SensorType"] = "Temperature";
    sensor1["SensorValue"] = temperature;

    JsonObject sensor2 = sensors.add<JsonObject>();
    sensor2["SensorType"] = "Humidity";
    sensor2["SensorValue"] = humidity;

    String jsonString;
    serializeJson(doc, jsonString);

    serializeJson(doc,Serial);

    int httpResponseCode = http.POST(jsonString);

    if (httpResponseCode > 0) {
      String response = http.getString();
      Serial.print("POST Response code: ");
      Serial.println(httpResponseCode);
      Serial.println(response);
    } else {
      Serial.print("Error on sending POST: ");
      Serial.println(httpResponseCode);
    }

    http.end(); // free resources
  }

  delay(5000); // send every 5 seconds
}
