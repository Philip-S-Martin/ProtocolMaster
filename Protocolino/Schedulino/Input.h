#ifndef INPUT_H
#define INPUT_H

#define FILE_INPUT 1

#include "Globals.h"

const byte input_state_count = 5;
byte input_state = 0;

// THE FIRST SECTION OF THIS FILE IS A FANCY SWITCH

void _Input_Start(), _Input_Cancel(), _Input_Event(), _Input_Reply(), _Input_Error(), _Input_Capacity();

// THESE ARRAYS MUST HAVE AN ENTRY FOR EACH POSSIBLE enum_state OR input_state!
// These are essentially labels for a matrix
enum_state Input_Map_States[enum_state_count] = {SETUP, RUNNING, RESET};
byte Input_Map_bytes[] = {'S', 'X', 'E', 'R', 'C'};
// And this is the matrix!
void (*Input_Map_Functions[enum_state_count][input_state_count])() = {
    {_Input_Start, _Input_Cancel, _Input_Event, _Input_Reply, _Input_Capacity},
    {_Input_Error, _Input_Cancel,  _Input_Error, _Input_Reply, _Input_Capacity},
    {_Input_Error, _Input_Error, _Input_Error, _Input_Error, _Input_Capacity}};

void Schedule_Input()
{
  if (Serial.available())
  {
    input_state = Serial.peek();
    for (byte i = 0; i < enum_state_count; i++)
    {
      if (state == Input_Map_States[i])
      {
        for (byte n = 0; n < input_state_count; n++)
        {
          if (input_state == Input_Map_bytes[n])
          {
            Input_Map_Functions[i][n]();
            return;
          }
        }
      }
    }
  }
  else return;
  // If we made it here, ther was something on the serial that wasn't recognized!
  Error(FILE_INPUT, 5, input_state);
  Serial.read();
}

void _Input_Start()
{
  Serial.read();
  state = RUNNING;
}
void _Input_Cancel()
{
  Serial.read();
  state = RESET;
}

long input_event_wait = 0;
void _Input_Event()
{
  if(Serial.available() < 7)
  {
    // wait for rest of event on serial line
    input_event_wait += elapsed;
    if(input_event_wait > 200)
    {
      Error(FILE_INPUT, 1, 0);
      Serial.flush();
      return;
    }
    return;
  }
  // reset wait timer, read 'E' to get to values
  input_event_wait = 0;
  Serial.read();

  long pow = 1 << 24;
  for(uint16_t i = 0; i < 4; i++)
  {
    schedule.time[schedule.end] += Serial.read() * pow;
    pow >> 8;
  }
  schedule.pin[schedule.end] = Serial.read();
  schedule.state[schedule.end] = Serial.read();
  schedule.end++;
  if(schedule.end >= SCHEDULE_MAX_EVENTS)
    Error(FILE_INPUT, 3, 0);
}
void _Input_Reply()
{
  Serial.read();
  Reply();
}
void _Input_Error()
{
  Serial.read();
  Error(FILE_INPUT, 2, input_state);
}
void _Input_Capacity()
{
  Serial.read();

}

#endif