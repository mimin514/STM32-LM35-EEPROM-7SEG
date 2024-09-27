#ifndef EEPROM_H_
#define EEPROM_H_

#include"stm32f1xx.h"

extern I2C_HandleTypeDef hi2c1;
void EEPROM_Read (uint16_t page, uint16_t offset, uint8_t *data, uint16_t size);
void EEPROM_Write (uint16_t page, uint16_t offset, uint8_t *data, uint16_t size);
void EEPROM_PageErase (uint16_t page);
void run_EEPROM(void);

 #endif /* EEPROM_H_ */
 