# Tycoon With Friends
This project is a multiplayer version of the game Tycoon from Persona 5 Royal using Unity and Mirror. I went along with a tutorial which was close enough to my project to help. 

# Rules
## Titles
Commoner - Everyone is a Commoner in the beginning of the game.
Tycoon - Someone who gets rid of their cards first. Has to give two of their cards to the beggar.
Rich - Someone who gets rid of their cards second. Has to give one of their cards to the poor.
Poor - Someone who gets rid of their cards third. Has to give their best cards to the rich.
Beggar - Loser of the round. Has to give 2 of their best cards to the Tycoon.

## Turns
- The First person to play determines how many cards others play. Ex. If the first player plays a single card then all players must play single cards. If they play pairs then all others play pairs. Same with more cards.
- If you don't want to play or can't play a card you can pass. If you have a playable card you wont be auto passed if you passed before.
- The turn ends when all players pass.

## Card Strength
- The cards strength from weakest to strongest is 3, 4, 5, 6, 7, 8, 9, 10, J, Q, K, A, 2, JOKER.
- Jokers: These are the highest cards and is considered a wild card in which you can add to other cards. Ex. You can have two 6's and a Joker and that would be considered three 6's or using 2 Jokers which is just 2 Jokers.
- 3 of Spades: The 3 of Spades is the only card that can beat a Joker.
- 8 Stops: If an 8 card is used the round ends and that player starts the turn.
- Revolutions and Counter-Revolutions: A revolution happens when you play 4 of the same card which flips the card strength from weakest to strongest being 2, A, K, Q, J, 10, 9, 8, 7, 6, 5, 4, 3, JOKER. If you make a revolution while already in a revolution it's called a Counter-Revolution which reverses the strength to normal.
