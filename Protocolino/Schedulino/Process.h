#ifndef PROCESS_H
#define PROCESS_H

#include "Globals.h"

#define FILE_PROCESS 2


// THE FIRST SECTION OF THIS FILE IS A FANCY SWITCH
void _Process_Setup(), _Process_Running(), _Process_Reset(), _Process_Done();

// The following is a map,
// THESE ARRAYS MUST HAVE AN ENTRY FOR EACH enum_state!
enum_state Process_Map_States[enum_state_count] = {SETUP, RUNNING, RESET};
void (*Process_Map_Functions[enum_state_count])() = {_Process_Setup, _Process_Running, _Process_Reset};

// This processes the global state using the Process Map
void Schedule_Process()
{
  for(char i = 0; i < enum_state_count; i++)
  {
    if(state == Process_Map_States[i])
    {
      Process_Map_Functions[i]();
      return;
    }
  }
  // If it made it to this point, the state is 
  // invalid because it is not in the map!
  Error(FILE_PROCESS, 1);
}

void _Process_Setup()
{
}

void _Process_Running()
{
  if(time > run_offset)
    run_time = time - run_offset;
  else
    run_time = UINT32_MAX - run_offset + time;
  while(schedule_first < schedule_end && schedule_time[schedule_first] <= run_time)
  {
    digitalWrite(schedule_pin[schedule_first], schedule_state_get(schedule_first));
    schedule_time[schedule_first] = run_time;
    schedule_first++;
  }
  if(schedule_first >= schedule_end)
  {
    _Process_Done();
  }
}

void _Process_Reset()
{
  _Process_Done();
  schedule_end = 0;
}

void _Process_Done()
{
  Done();
  for(schedule_first = 0; schedule_first < schedule_end; schedule_first++)
  {
    Report(schedule_first);
  }
  schedule_first = 0;
  run_time = 0;
  run_offset = 0;

  for(char i = 2; i < 19; i++)
  {
    pinMode(i, OUTPUT);
    digitalWrite(i, LOW);
  }

  state = SETUP;
  Done();
}

#endif