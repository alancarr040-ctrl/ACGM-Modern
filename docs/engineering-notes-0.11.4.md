# Engineering Notes - 0.11.4

## Package

ACGM Modern Client 0.11.4 - Tree Parser & Awards Mapping Fix

## Philosophy

Recreation first. Modernization underneath.

## Legacy tree parser confirmation

The legacy VB6 client loads the allegiance tree by splitting `HTTP.strHTTPData` on `!;` and then splitting each character record on `|`. The modern client preserves that behavior exactly.

Tree record format:

```text
id|name|level|rank|patron_id|vassal_ids|is_mule|is_pk|is_rescue_squad_member
```

VB6 source confirmation:

```vb
objCharDB.Add Val(strCharInfo(0)), strCharInfo(1), _
  Val(strCharInfo(2)), Val(strCharInfo(3)), Val(strCharInfo(4)), _
  strCharInfo(5), Val(strCharInfo(6)), Val(strCharInfo(7)), Val(strCharInfo(8)), _
  Trim("Char" & strCharInfo(0))
```

The `Characters.Add` signature maps those final fields as:

```vb
iIsMule, iIsPK, iIsRescueSquadMember
```

Therefore the correct modern mapping is:

- Field 6 => Mule
- Field 7 => PK
- Field 8 => Rescue Squad Member

William Ohmsford test record:

```text
2|William Ohmsford|126|3|1|11,12,13,14,15,16,41,42,43,44,45|0|0|1
```

Expected result:

- IsMule = false
- IsPK = false
- IsRescueSquad = true
- Icon = Rescue

## Icon priority

Legacy VB6 `AddNode` chooses icons in this order:

1. Mule
2. Rescue Squad
3. PK
4. Normal

The modern client preserves that priority.

## Awards mapping

Awards are not part of the tree payload. They are parsed from the final field of the character-detail payload.

For the William Ohmsford diagnostic payload, the final field is:

```text
Monster Smasher
```

Modern display converts legacy `!;` separated award entries into one award per display line.

## Diagnostics retained

The package retains diagnostics while this mapping is verified:

- `logs/tree-payload-diagnostics.log`
- `logs/character-flags-awards-diagnostics.log`

Tree diagnostics now include the raw final tree fields as `RawFlags='field6|field7|field8'` beside the interpreted flags and selected icon.

## Protocol compatibility

No protocol changes were made.

Unchanged:

- msgid values
- POST field names
- request format
- response format
- legacy `!;` tree record splitting
