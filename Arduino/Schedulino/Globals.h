#ifndef GLOBALS_H
#define GLOBALS_H

#define FILE_GLOBALS 4

// STATES
const byte enum_state_count = 3;
typedef enum
{
  SETUP = 0,
  RUNNING,
  RESET
} enum_state;
enum_state state = SETUP;

// SCHEDULE
uint32_t time = 0;
uint32_t elapsed = 0;

uint32_t run_time = 0;
uint32_t run_offset = 0;

void _schedule_init();
uint16_t _schedule_capacity(), _schedule_first_index(), _schedule_last_index();

#define SCHEDULE_MAX_EVENTS 192
typedef struct
{
  uint16_t first = 0;
  uint16_t last = 0;
  uint16_t (*capacity)();
  uint16_t (*firstIndex)();
  uint16_t (*lastIndex)();
  uint32_t time[SCHEDULE_MAX_EVENTS];
  byte pin[SCHEDULE_MAX_EVENTS];
  byte state[SCHEDULE_MAX_EVENTS];
} schedule_data;
schedule_data schedule;

void _schedule_init()
{
  schedule.capacity = _schedule_capacity;
  schedule.firstIndex = _schedule_first_index;
  schedule.lastIndex = _schedule_last_index;
}
uint16_t _schedule_capacity()
{
  return SCHEDULE_MAX_EVENTS - (schedule.last - schedule.first);
}
uint16_t _schedule_first_index()
{
  return schedule.first % SCHEDULE_MAX_EVENTS;
}
uint16_t _schedule_last_index()
{
  return schedule.last % SCHEDULE_MAX_EVENTS;
}
uint16_t _schedule_index(uint16_t index)
{
  return index % SCHEDULE_MAX_EVENTS;
}

// SERIAL HELPERS

/*
void WriteBytes(uint32_t target)
{
  byte sendBuf[4];
  sendBuf[3] = (byte)target & 255;
  sendBuf[2] = (byte)(target >> 8) & 255;
  sendBuf[1] = (byte)(target >> 16) & 255;
  sendBuf[0] = (byte)(target >> 24) & 255;
  Serial.write(sendBuf, 4);
}

void WriteBytes(uint16_t target)
{
  byte sendBuf[2];
  sendBuf[1] = (byte)target & 255;
  sendBuf[0] = (byte)(target >> 8) & 255;
  Serial.write(sendBuf, 2);
}

void WriteBytes(byte target)
{
  Serial.write((byte)target);
}
*/
// SERIAL IO

void Error(byte file, byte error, byte ext)
{
  Serial.write((byte)'E');
  Serial.println(file);
  Serial.println(error);
  Serial.println(ext);
}

void Reply()
{
  Serial.write((byte)'R');
}

void Done()
{
  Serial.write((byte)'D');
  Serial.println(run_time);
  Serial.println((byte)state);
}

void Report(uint16_t index)
{
  uint16_t realIndex = _schedule_index(index);
  Serial.write((byte)'P');
  Serial.println(index);
  Serial.println(schedule.time[realIndex]);
  Serial.println(schedule.pin[realIndex]);
  Serial.println(schedule.state[realIndex]);
}

void Capacity()
{
  Serial.write((byte)'C');
  Serial.println(schedule.capacity());
  Serial.println((byte)(63 - Serial.available()));
}

#endif