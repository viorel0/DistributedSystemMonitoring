#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>

#define PPM_PIN 33 

// WiFi credentials
const char* ssid = "x";
const char* password = "x";

const char* serverUrl = "http://x:5101/api/Node/upload";

void setup() {
  Serial.begin(115200);
  
  WiFi.begin(ssid, password);
  Serial.print("Connecting to WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nConnected!");
}

void loop() {
  float RL = 9.89; // 9.89 kohm
  float R0= 4.98;
  int analogValue = analogRead(PPM_PIN);
  float Vout = analogValue * (3.3 / 4095.0);
  float Rs = ((3.3 - Vout) * RL) / Vout;

  float ratio = Rs / R0;

  //https://davidegironi.blogspot.com/2014/01/cheap-co2-meter-using-mq135-sensor-with.html
  float ppm = 116.6020682 * pow(ratio, -2.769034857);

  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;

    http.begin(serverUrl);
    http.addHeader("Content-Type", "application/json");

    JsonDocument doc;
    doc["NodeName"] = "ESP32_2";

    JsonArray sensors = doc.createNestedArray("Sensors");

    JsonObject sensor1 = sensors.add<JsonObject>();
    sensor1["SensorType"] = "MQ-135";
    sensor1["SensorValue"] = ppm;

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
