#include "Schedulino.h"

/*

*/

void setup()
{
  Serial.begin(9600);
  _schedule_init();
  _Process_Init();
}

void loop()
{
  Schedule_Input();

  long pretime = time;
  time = millis();
  if(time > pretime)
    elapsed = time - pretime;
  else
    elapsed = UINT32_MAX - pretime + time;

  Schedule_Process();
}
