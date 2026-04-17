 \ ******************************************************************
 \    it is written for Arduino lcd keypad shield                   *
 \    it shows the button pressed on lcd                            *
 \    the program also demonstrates the use of adc                  *
 \    all buttons are connected to A0 analog pin for adc            *
 \    press ESC to exit the loop in program                         *
 \                                                                  *
 \                                                   10.04.2026     *                        
 \ ******************************************************************


marker -lcd
flash
$23 constant PINB
$24 constant DDRB
$25 constant PORTB
$26 constant PINC
$27 constant DDRC
$28 constant PORTC
$29 constant PIND
$2a constant DDRD
$2b constant PORTD

$1 constant lcd.rs
$2 constant lcd.enable
: lcd_init_pins
  lcd.enable DDRB mset
  lcd.enable PORTB mclr
  lcd.rs DDRB mset
  lcd.rs PORTB mclr
  DDRD c@ %11110000 or DDRD c!
;
: nop ;
: lcd_enable
  lcd.enable PORTB mset
  20 for next nop
  lcd.enable PORTB mclr
;
: write-hnibble ( n -- )
  $f0 and PORTD c@ $0f and or PORTD c!
  lcd_enable
;
: write-byte ( n -- )
  dup write-hnibble
  20 for next nop
  4 lshift write-hnibble
  20 for next nop
;
: lcd_command ( n -- )
  lcd.rs PORTB mclr
  write-byte
;
: lcd_data ( n -- )
  lcd.rs PORTB mset
  write-byte
;
: lcd_init
  lcd_init_pins
  #5 ms
  $05 $01 $06 $0c $28 $32 $33 2dup
  8 for lcd_command next ms
;
: lcd_home %10 lcd_command ;
: lcd_line1 $80 lcd_command ;
: lcd_line2 $c0 lcd_command ;
: lcd_xy1 ( col -- ) 1 - $80 + lcd_command ;
: lcd_xy2 ( col -- ) 1 - $c0 + lcd_command ;
: lcd_clear $01 lcd_command ;
: lcd_putc ( n -- ) lcd_data ;
: lcd_puts for c@+ lcd_putc next drop ;

: init_pins
  $00 DDRC c! \ PORTC input
  $3f PORTC c! \ with internal pullup
  $04 dup DDRD mset PORTD mclr
  $08 dup DDRD mset PORTD mclr
  $08 dup DDRB mset PORTB mclr
  $10 dup DDRB mset PORTB mclr
  $20 dup DDRB mset PORTB mclr
;

marker -read-adc
flash
$78 constant adcl
$79 constant adch
$7a constant adcsra
$7b constant adcsrb
$7c constant admux
$7e constant didr0
\ Bit masks
%10000000 constant mADEN
%01000000 constant mADSC
%00010000 constant mADIF

: adc.clear.iflag ( -- )
  mADIF adcsra mset \ clear by writing 1
;

: adc.init ( -- )
  $3f didr0 c! \ Disable digital inputs 5 through 0
  $40 admux c! \ AVcc as ref , right - adjust result , channel 0
  $06 adcsra c! \ single conversion mode , prescaler 64
  mADEN adcsra mset \ enable ADC
  adc.clear.iflag
;

: adc.close ( -- )
  mADEN adcsra mclr
  adc.clear.iflag
;

: adc.wait ( -- )
  begin mADSC adcsra mtst 0= until
;

: adc.select ( u -- )
  adc.wait
  $0f and \ channel selected by lower nibble
  admux c@ $f0 and \ fetch upper nibble
  or admux c!
;

: adc@ ( -- u )
  adc.wait
  mADSC adcsra mset
  adc.wait
  adcl c@ adch c@ #8 lshift or
  adc.clear.iflag
;

marker -poll
flash
-5 constant nokey
$1 constant key-select
$2 constant key-left
$3 constant key-up
$4 constant key-down
$5 constant key-right

: poll-key ( -- keynum )
  adc.init 0 adc.select adc@ adc.close \ adc0
  dup 1000 > if drop nokey exit then
  dup dup 600 > swap 735 < and if drop key-select exit then
  dup dup 370 > swap 490 < and if drop key-left   exit then
  dup dup 60 > swap 145 < and if drop key-up      exit then
  dup dup 210 > swap 320 < and if drop key-down   exit then
  dup dup -1 > swap 70 < and if drop key-right    exit then
  drop nokey
;

ram 0 value stop flash

: prn-key-select s" key-select" ; \ to print on lcd when pressed
: prn-key-left s" key-left" ;
: prn-key-right s" key-right" ;
: prn-key-up s" key-up" ;
: prn-key-down s" key-down" ;

: key-test ( -- )
  500 ms
	init_pins
	lcd_init	
  50 ms
	false to stop
  begin
    poll-key 
    dup nokey = if lcd_clear 50 ms then
    dup key-select = if prn-key-select lcd_puts 100 ms then
    dup key-left = if prn-key-left lcd_puts 100 ms then
    dup key-up = if prn-key-up lcd_puts 100 ms then
    dup key-down = if prn-key-down lcd_puts 100 ms then
    key-right = if prn-key-right lcd_puts 100 ms then
    #50 ms		
		key? if 
			key #27 = if true to stop then \ ESC key
		then
		500 ms
  stop until
;
