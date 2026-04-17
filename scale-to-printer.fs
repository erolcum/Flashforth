 \ ******************************************************************
 \    it is written for Arduino Pro micro                           *
 \    It sends the data received from the scale via the serial      *
 \    port (uart) to the label printer. Every time data is          *                                           
 \    received from the scale, a label containing the weight        *
 \    information is printed. rx0 is connected to the scale         *
 \    or checkweigher. tx0 is connected to label printer.           *
 \                                                                  * 
 \    The Arduino Uno has a single serial port. This port is also   *
 \    used for Flashforth. So we can not use it to connect to a     *
 \    scale. The Pro Micro, however, has a separate serial port.    *
 \    You may also use Arduino Mega, since it has many ports.       *
 \                                                                  *
 \    The program also demonstrates the use of a buffer in RAM      *
 \    and a data block in flash memory.                             *
 \    to run the program type the following line to terminal        *
 \    then press enter  :                                           *
 \    listen-rx0                                                    *
 \    press ESC to exit the loop in program                         *
 \                                                                  *
 \                                                   07.04.2026     *                        
 \ ******************************************************************
 
marker -scale
\ #!A1#DC#IMSR50/50#ERN/1//0#T5#J5#YN101/0B/82///
flash
create first
  char # c,  char ! c,  char A c,  char 1 c,  \ #!A1
  char # c,  char D c,  char C c,  char # c,      
  char I c,  char M c,  char S c,  char R c, 
  char 5 c,  char 0 c,  char / c,  char 5 c,  
  char 0 c,  char # c,  char E c,  char R c,                   
  char N c,  char / c,  char 1 c,  char / c, 
  char / c,  char 0 c,  char # c,  char T c,
  char 5 c,  char # c,  char J c,  char 5 c, 
  char # c,  char Y c,  char N c,  char 1 c,  
  char 0 c,  char 1 c,  char / c,  char 0 c,  
  char B c,  char / c,  char 8 c,  char 2 c,  
  char / c,  char / c,  char / c,   

create last
  char g c,  char r c,  char # c,  char G c,
  char # c,  char Q c,  char 1 c,  
	
ram
create buf 150 allot      \ buffer to be sent to printer
create buf-scale 16 allot \ buffer for data from scale
0 value length            \ total length of buffer
0 value stop

: buf-reset 0 to length ;
: buf-add ( addr u -- )
	2dup
	buf length +
	swap cmove
	length + to length
	drop
;

marker -send
: >printer  ( addr len -- )
	for
		dup c@ tx0 1+
	next drop
	$0d tx0 $0a tx0
;

: listen-rx0 ( -- )
	decimal
	207 $cc ! \ baud 9600
	false to stop
	begin
		rx0? if
			rx0 $02 = if
					buf-scale 8 for
          begin rx0? until rx0 
          over c! 1+          
					next drop
					buf-reset
					first 47 buf-add
					buf-scale 8 buf-add
					last 7 buf-add
					buf length >printer
			then
		then
		key? if 
			key 27 = if true to stop then \ ESC key
		then
	pause
	stop until
;




