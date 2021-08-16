using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for tiles
/// </summary>
public class Tile : Interior
{
	/// <summary>
	/// Method for converting tilemap coordinates into world coordinates
	/// </summary>
	/// <param name="tranTile">Tilemap coordinates</param>
	/// <returns>World coordinates</returns>
	public static Vector2 TileToTrans(Vector2 tranTile)
	{
		Vector2 result;
		result.x = tranTile.x + tranTile.y;
		result.y = -0.5f * tranTile.x + 0.5f * tranTile.y;
		return result;
	}
	
	/// <summary>
	/// Method for converting world coordinates into tilemap coordinates
	/// </summary>
	/// <param name="trans">World coordinates</param>
	/// <returns>Tilemap coordinates</returns>
	public static Vector2 TransToTile(Vector2 trans)
	{
		Vector2 result;
		result.x = (float)Math.Ceiling(trans.x / 2 - trans.y);
		result.y = (float)Math.Floor(trans.x / 2 + trans.y);
		return result;
	}
	
	/// <summary>
	/// Method for converting world coordinates into non-integers tilemap coordinates
	/// </summary>
	/// <param name="trans">World coordinates</param>
	/// <returns>Non-integers tilemap coordinates</returns>
	public static Vector2 TransToTileF(Vector2 trans)
	{
		Vector2 result;
		result.x = trans.x / 2 - trans.y;
		result.y = trans.x / 2 + trans.y;
		return result;
	}
	
	/// <summary>
	/// Sprite of the tile
	/// </summary>
	private SpriteRenderer _sprite;

	/// <summary>
	/// Property for an InteriorType of the tile
	/// </summary>
	public override InteriorType TileType
	{
		get => PTileType;
		set
		{
			PTileType = value;
			_sprite.sortingLayerName = value == InteriorType.Floor ? "Floor" : "Objects";
		}
	}
	
	/// <summary>
	/// Sets sprite to the tile
	/// </summary>
	/// <param name="spriteNum">Number of the sprite</param>
	public override void SetSprite(int spriteNum)
	{
		if(name == "TileUp")
			GetComponent<SpriteRenderer>().sprite = Map.Sprites[spriteNum%240];
			
		else
		{
			if (spriteNum > 239)
				spriteNum -= 16;
			GetComponent<SpriteRenderer>().sprite = Map.Sprites[spriteNum%224];
		}
	}
}
