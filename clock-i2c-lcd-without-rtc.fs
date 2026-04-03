 \ ******************************************************************
 \    for Arduino Uno/Nano/Mega/Pro mini/Pro micro                  *
 \    clock with LCD1602 i2c interface                              *
 \    the clock works without RTC module                            *                                           
 \    program requires case.fs in FF5.0\forth and                   *
 \       i2c-base.fs in FF5.0\avr\forth                             *
 \                                                                  *
 \    i2c interface is adapted from Scamp3 to Arduino               *
 \    source videos :                                               *
 \    https://www.youtube.com/watch?v=TaXCTG5UmaU                   *
 \    https://www.youtube.com/watch?v=q1ImsmANyfE                   *
 \    program also shows LCD CGRAM usage to add a user character    *
 \    to run the program or to set the clock, write the following   *
 \    line to terminal then press enter  :                          *
 \    14 to h 5 to m run-clock                                      *
 \    14:05 appears on lcd, it starts to running                    *
 \                                                                  *
 \    my blog page on this topic :                                  *
 \    https://erolcum.blogspot.com/2026/03/flashforth-tantm.html    *
 \    you can translate my page to your language by using the       *
 \    translate widget on the left side                             * 
 \                                                   02.04.2026     *                        
 \ ******************************************************************
 
      -LCD-
    marker -LCD-

   $27 constant addr \ I2C interface address
	 decimal
 : >i2c     ( char -- )
	   i2c.init addr i2c.write i2c.c! i2c.stop ; 	
           
 : >lcd    ( char -- )  \ data
           dup
           $f0 and dup dup
           $9 or  >i2c
           $d or  >i2c
           $9 or  >i2c
           $0f and 4 lshift dup dup
           $9 or  >i2c
           $d or  >i2c
           $9 or  >i2c ;

 : >cmd    ( char -- )   \ command
           dup
           $f0 and dup dup
           $8 or  >i2c
           $c or  >i2c
           $8 or  >i2c
           $0f and 4 lshift dup dup
           $8 or  >i2c
           $c or  >i2c
           $8 or  >i2c
           1 ms ;

 : >init    ( char -- )
           dup
           $f0 and dup dup
           $8 or  >i2c
           $c or  >i2c
           $8 or  >i2c
           5 ms
           $0f and 4 lshift dup dup
           $8 or  >i2c
           $c or  >i2c
           $8 or  >i2c
           1 ms ;

 : lcd.init
             100 ms
             $33 >init
             $32 >init       \ 4 bit
             $28 >cmd        \ 2 lines 5X7
             $0e >cmd        \ Blink OFF
             $01 >cmd 5 ms ; \ Home - CLR screen

  \ Display commands
 : CLR    $01 >cmd 5 ms ;  \ clear screen home cursor
 : CURL   $10 >cmd  ;      \ cursor left
 : CURR   $14 >cmd  ;      \ cursor right
 : H1     $80 >cmd  ;      \ home cursor line 1
 : H2     $c0 >cmd  ;      \ home cursor line 2
 : C0     $0c >cmd  ;      \ cursor OFF (underline)
 : C1     $0e >cmd  ;      \ cursor ON  (underline)
 : BC1    $0f >cmd  ;      \ blink cursor ON  (block)
 : BC0    $0e >cmd  ;      \ blink cursor OFF (block)
 : DL     $18 >cmd  ;      \ shift display LEFT
 : DR     $1c >cmd  ;      \ shift display RIGHT

 : setcursor ( line col -- )
             swap 1 =
             if    $80 +
             else  $c0 +
             then >cmd ;

marker -cgram
flash
bin
create bar1
  11100 c, 11110 c, 11110 c, 11110 c, 11110 c, 11110 c, 11110 c, 11100 c,
create bar2
  00111 c, 01111 c, 01111 c, 01111 c, 01111 c, 01111 c, 01111 c, 00111 c,
create bar3
  11111 c, 11111 c, 00000 c, 00000 c, 00000 c, 00000 c, 11111 c, 11111 c,
create bar4
  11110 c, 11100 c, 00000 c, 00000 c, 00000 c, 00000 c, 11000 c, 11100 c,
create bar5
  01111 c, 00111 c, 00000 c, 00000 c, 00000 c, 00000 c, 00011 c, 00111 c,
create bar6
  00000 c, 00000 c, 00000 c, 00000 c, 00000 c, 00000 c, 11111 c, 11111 c,
create bar7
  00000 c, 00000 c, 00000 c, 00000 c, 00000 c, 00000 c, 00111 c, 01111 c,
create bar8
  11111 c, 11111 c, 00000 c, 00000 c, 00000 c, 00000 c, 00000 c, 00000 c, 
ram
decimal
: cgram ( addr char-id -- )
  8 * $40 + >cmd   
  8 for                     
      dup 7 r@ - + c@ >lcd         
    next
  drop
  $80 >cmd
; 

: send-to-cgram
  bar1 0 cgram
  bar2 1 cgram 
  bar3 2 cgram
  bar4 3 cgram
  bar5 4 cgram
  bar6 5 cgram
  bar7 6 cgram
  bar8 7 cgram  
;

marker -digits
: line1 dup 1 swap setcursor ;
: line2 2 swap setcursor ;
: custom0 ( col -- )
	line1 1 >lcd 7 >lcd 0 >lcd line2 1 >lcd 5 >lcd 0 >lcd ;
: custom1 ( col -- )
	line1 32 >lcd 32 >lcd 0 >lcd line2 32 >lcd 32 >lcd 0 >lcd ;
: custom2 ( col -- )
	line1 4 >lcd 2 >lcd 0 >lcd line2 1 >lcd 5 >lcd 5 >lcd ;
: custom3 ( col -- )
	line1 4 >lcd 2 >lcd 0 >lcd line2 6 >lcd 5 >lcd 0 >lcd ;
: custom4 ( col -- )
	line1 1 >lcd 5 >lcd 0 >lcd line2 32 >lcd 32 >lcd 0 >lcd ;
: custom5 ( col -- )
	line1 1 >lcd 2 >lcd 3 >lcd line2 6 >lcd 5 >lcd 0 >lcd ;
: custom6 ( col -- )
	line1 1 >lcd 2 >lcd 3 >lcd line2 1 >lcd 5 >lcd 0 >lcd ;	
: custom7 ( col -- )
	line1 1 >lcd 7 >lcd 0 >lcd line2 32 >lcd 32 >lcd 0 >lcd ;
: custom8 ( col -- )
	line1 1 >lcd 2 >lcd 0 >lcd line2 1 >lcd 5 >lcd 0 >lcd ;
: custom9 ( col -- )
	line1 1 >lcd 2 >lcd 0 >lcd line2 6 >lcd 5 >lcd 0 >lcd 
;

marker -printnum
: print-number ( value col -- )
	swap 
	case
	0 of custom0 endof
	1 of custom1 endof
	2 of custom2 endof
	3 of custom3 endof
	4 of custom4 endof
	5 of custom5 endof
	6 of custom6 endof
	7 of custom7 endof
	8 of custom8 endof
	9 of custom9 endof
	endcase
;

ram
0 value s
0 value m
0 value h
0 value previousTicks
0 value previousSec
0 value colonStat

: update-clock ( -- )
  ticks previousTicks - ( ticks difference )
  
  1000 u< invert if  ( 1000 u>= if )
    s 1+ to s
    previousTicks 1000 + to previousTicks
    
    s 59 > if
      0 to s
      m 1+ to m
    then
    
    m 59 > if
      0 to m
      h 1+ to h
    then
    
    h 23 > if
      0 to h
    then
  then 
;
	
: update-lcd ( -- )
  h 10 / 0 print-number
  h 10 u/mod drop 3 print-number  
  m 10 / 9 print-number
  m 10 u/mod drop 12 print-number
;

: colon-on
	1 7 setcursor 46 >lcd   \ print .
	2 7 setcursor 46  >lcd
;	
: colon-off
	1 7 setcursor 32 >lcd   \ print space
	2 7 setcursor 32 >lcd	
;	
: run-clock ( -- )
	lcd.init
	send-to-cgram
	C0 update-lcd
	ticks to previousTicks
  begin
    update-clock
		s previousSec <> if
			colonStat if colon-off false to colonStat 
			else colon-on true to colonStat then
			s 0= if update-lcd then   
				s to previousSec
		then 		
	100 ms	
  again 
;

  
