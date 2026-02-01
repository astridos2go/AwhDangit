# v1.0.1

- Added missing `StaticNetcodeLib` dependency string to plugin, fixing an issue where only the host could see or be
  affected by the gambling punishment
- Updated reset logic to only update players with non-zero gambling profits.
- Fixed an issue where chips gambled in Roulette, Slots, and The Wheel would despawn before being applied to the
  player's gambling profit (allowed players to gamble chips on those games with no consequences)
- Fixed an issue where players were punished after losing in Blackjack before the animations finished.

# v1.0.0
- Initial release