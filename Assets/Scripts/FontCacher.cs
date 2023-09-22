using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class FontCacher : MonoBehaviour {

	public TMP_Text TextField; 
	public enum GlyphSets{
		Custom,
		Current,
		NumbersOnly,
		Standard
	}
	public GlyphSets GlyphSet = GlyphSets.Standard;
	public string CustomGlyphs;

	//protected void Awake () {
	//	if(TextField==null)
	//	{
	//		TextField = gameObject.GetComponent<TMP_Text>();
	//	}
	//	StartCoroutine(CacheFont());
	//}

    protected IEnumerator CacheFont()
	{
		string glyphs;

		switch(GlyphSet)
		{
		case(GlyphSets.Custom):
			glyphs = CustomGlyphs;
			break;
		case(GlyphSets.Current):
			glyphs = TextField.text;
			break;
		case(GlyphSets.NumbersOnly):
			glyphs ="0123456789";
			break;
		case(GlyphSets.Standard):
		default:
			glyphs = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_+=~`[]{}|\\:;\"'<>,.?/ ";
			break;
		}

		TMP_FontAsset font = TextField.font;
        //font.RequestCharactersInTexture(glyphs, TextField.fontSize, TextField.fontStyle);
        yield break;
	}
}
