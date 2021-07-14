#include <Adafruit_VS1053.h>
#include <SPI.h>
#include <SD.h>
#include <EEPROM.h>                                              //Include the EEPROM Library in this sketch.



// These are the pins used for the music maker shield
#define SHIELD_RESET  -1      // VS1053 reset pin (unused!)
#define SHIELD_CS     7      // VS1053 chip select pin (output)
#define SHIELD_DCS    6      // VS1053 Data/command select pin (output)

// These are common pins between breakout and shield
#define CARDCS 4     // Card chip select pin
// DREQ should be an Int pin, see http://arduino.cc/en/Reference/attachInterrupt
#define DREQ 3       // VS1053 Data request, ideally an Interrupt pin

Adafruit_VS1053_FilePlayer musicPlayer = 
  Adafruit_VS1053_FilePlayer(SHIELD_RESET, SHIELD_CS, SHIELD_DCS, DREQ, CARDCS);

//a is MSB, d is LSB
int tone_a = A2;
int tone_b = A3;
int tone_c = A0;
int tone_d = A1;
int trigger = A4;
int music_out = A5; //output from music board to main arduino to state when music is playing 

int read_a=0;
int read_b=0;
int read_c=0;
int read_d=0;
int read_trigger=0;

int tune_number=0;
String tune="";

  
String machineGun = "guns.mp3";
String twitter = "twtr.mp3";
String ninekhz = "9khz.mp3";
String fourkhz = "4khz.mp3";
String twokhzten = "2k10s.mp3";
String ninekhzten = "9k10s.mp3";
String war_zone = "warzone.mp3";



char track[15] = "9khz.mp3";
char trackb[15] = "guns.mp3";




void setup() 
{
    pinMode(tone_a,INPUT);
    pinMode(tone_b,INPUT);
    pinMode(tone_c,INPUT);
    pinMode(tone_d,INPUT);
    pinMode(trigger,INPUT);
    pinMode(music_out,OUTPUT);

    Serial.begin(9600); 

     if (! musicPlayer.begin()) { // initialise the music player
     //Serial.println(F("Couldn't find VS1053, do you have the right pins defined?"));
     while (1);
  }
  //Serial.println(F("VS1053 found"));
  SD.begin(CARDCS);    // initialise the SD card
  
  // Set volume for left, right channels. lower numbers == louder volume!
  musicPlayer.setVolume(20,20);

  // If DREQ is on an interrupt pin (on uno, #2 or #3) we can do background
  // audio playing
  musicPlayer.useInterrupt(VS1053_FILEPLAYER_PIN_INT);  // DREQ int
  randomSeed(analogRead(0));
    
}

void loop() 
{
  digitalWrite(music_out,LOW);
  if(digitalRead(trigger))
  {
    delay(30);
    read_a=digitalRead(tone_a);
    read_b=digitalRead(tone_b);
    read_c=digitalRead(tone_c);
    read_d=digitalRead(tone_d);
    
    tune_number=8*read_d + 4*read_c + 2*read_b + 1*read_a;
    
    switch(tune_number)
    {
      case 0:
      machineGun.toCharArray(track,15);
      musicPlayer.setVolume(20,20);
      break;
 
      case 1:
      twitter.toCharArray(track, 15);
      musicPlayer.setVolume(20,20);
      break;

      case 2:
      ninekhz.toCharArray(track, 15);
      musicPlayer.setVolume(50,50);
      break;

      case 3:
      fourkhz.toCharArray(track, 15);
      musicPlayer.setVolume(50,50);
      break;
      
      case 4:
      twokhzten.toCharArray(track, 15);
      musicPlayer.setVolume(50,50);
      break;

      case 5:
      ninekhzten.toCharArray(track, 15);
      musicPlayer.setVolume(50,50);
      break;

      case 6:
      war_zone.toCharArray(track, 15);
      musicPlayer.setVolume(20, 20);
      
      break;

      case 7:
      
      break;

      case 8:
      
      break;

      case 9:
      
      break;

      case 10:
      
      break;

      case 11:
      
      break;

      case 12:
      
      break;

      case 13:
      
      break;

      case 14:
      
      break;

      case 15:
      
      break;

      
    }
    Serial.println(tune_number);

    digitalWrite(music_out, HIGH);

    while(digitalRead(trigger))
    {
      if(!musicPlayer.playingMusic)
          musicPlayer.startPlayingFile(track);
      else
        delay(5);
    }
    musicPlayer.stopPlaying(); 
    
    digitalWrite(music_out, LOW);
  }

  

}
