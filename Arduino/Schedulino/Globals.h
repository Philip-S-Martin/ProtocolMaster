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

// SERIAL HELPERS

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

// SERIAL IO

void Error(byte file, byte error, byte ext)
{
  WriteBytes((byte)'E');
  WriteBytes(file);
  WriteBytes(error);
  WriteBytes(ext);
}

void Reply()
{
  WriteBytes((byte)'R');
  WriteBytes((byte)Serial.available());
}

void Done()
{
  WriteBytes((byte)'D');
  WriteBytes(run_time);
  WriteBytes((byte)state);
}

void Report(uint16_t index)
{
  WriteBytes((byte)'P');
  WriteBytes(index);
  WriteBytes(schedule.time[index]);
  WriteBytes(schedule.pin[index]);
  WriteBytes(schedule.state[index]);
}

void Capacity()
{
  WriteBytes((byte)'C');
  WriteBytes(schedule.capacity());
}

#endif