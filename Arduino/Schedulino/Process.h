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
  for (byte i = 0; i < enum_state_count; i++)
  {
    if (state == Process_Map_States[i])
    {
      Process_Map_Functions[i]();
      return;
    }
  }
  // If it made it to this point, the state is
  // invalid because it is not in the map!
  Error(FILE_PROCESS, 1, 0);
}

void _Process_Setup()
{
}

void _Process_Running()
{
  if (time > run_offset)
    run_time = time - run_offset;
  else
    run_time = UINT32_MAX - run_offset + time;

  uint16_t firstIndex = schedule.firstIndex();

  while (schedule.first < schedule.last && schedule.time[firstIndex] <= run_time)
  {
    digitalWrite(schedule.pin[firstIndex], schedule.state[firstIndex]);
    schedule.time[firstIndex] = run_time;
    Report(schedule.first);
    schedule.first++;
    firstIndex = schedule.firstIndex();
  }
  if (schedule.first >= schedule.last)
  {
    _Process_Done();
  }
}

void _Process_Reset()
{
  schedule.first = 0;
  schedule.last = 0;
  run_time = 0;
  run_offset = 0;

  for (byte i = 2; i < 19; i++)
  {
    pinMode(i, OUTPUT);
    digitalWrite(i, LOW);
  }

  state = SETUP;
  Capacity();
}

void _Process_Done()
{
  state = RESET;

  Capacity();
  schedule.first = 0;
  schedule.last = 0;
  Done();
  run_time = 0;
  run_offset = 0;

  for (byte i = 2; i < 19; i++)
  {
    pinMode(i, OUTPUT);
    digitalWrite(i, LOW);
  }
  
  state = SETUP;
}

#endif