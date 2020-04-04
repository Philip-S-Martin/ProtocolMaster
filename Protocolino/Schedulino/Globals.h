#ifndef GLOBALS_H
#define GLOBALS_H

#define FILE_GLOBALS 4

// STATES
const char enum_state_count = 3;
typedef enum 
{
  SETUP,
  RUNNING,
  RESET
} enum_state;
enum_state state = SETUP;

// SCHEDULE
unsigned long time = 0;
unsigned long elapsed = 0;

unsigned long run_time = 0;
unsigned long run_offset = 0;

short schedule_first = 0;
short schedule_end = 0;

#define SCHEDULE_MAX_EVENTS 192
unsigned long schedule_time[SCHEDULE_MAX_EVENTS];
unsigned char schedule_pin[SCHEDULE_MAX_EVENTS];
char _schedule_state[SCHEDULE_MAX_EVENTS/8];

void schedule_state_set(short i, bool value)
{
  char flag = 1 << (i & 7);
  i = i >> 3;
  _schedule_state[i] = _schedule_state[i] & flag;
}

bool schedule_state_get(short i)
{
  char shift = (i & 7);
  i = i >> 3;
  return (_schedule_state[i] >> shift) & 1;
}

// SERIAL IO

void Error(byte file, short error)
{
  Serial.write("E");
  Serial.write(file);
  Serial.write(error);
}

void Reply()
{
  Serial.write("R");
  Serial.write((byte)Serial.available());
}

void Done()
{
  Serial.write("D");
  Serial.write(run_time);
  Serial.write(state);
}

void Report(short index)
{
  Serial.write("P");
  Serial.write(index);
  Serial.write(schedule_time[index]);
  Serial.write(schedule_pin[index]);
  Serial.write(schedule_state_get(index));
}

#endif