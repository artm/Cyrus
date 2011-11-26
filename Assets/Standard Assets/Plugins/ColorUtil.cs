using UnityEngine;

public class ColorUtil {
	// r,g,b values are from 0 to 1
	// h = [0,360], s = [0,1], v = [0,1]
	public static void RGBtoHSV( float r, float g, float b, out float h, out float s, out float v )
	{
		float min = Mathf.Min( r, g, b );
		float max = Mathf.Max( r, g, b );
		v = max;
		float delta = max - min;
		s = (max != 0) ? delta / max : 0;
		if (delta == 0)
			h = 0;
		else if( r == max )
			h = ( g - b ) / delta; // between yellow & magenta
		else if( g == max )
			h = 2 + ( b - r ) / delta; // between cyan & yellow
		else
			h = 4 + ( r - g ) / delta; // between magenta & cyan
		h *= 60; // degrees
		if( h < 0 )
			h += 360;
	}

	public static void HSVtoRGB( float h, float s, float v, out float r, out float g, out float b )
	{
		if( s == 0 ) {
			// achromatic (grey)
			r = g = b = v;
			return;
		}
		h /= 60; // sector 0 to 5
		int i = Mathf.FloorToInt( h );
		float f, p, q, t;
		f = h - i; // factorial part of h
		p = v * ( 1 - s );
		q = v * ( 1 - s * f );
		t = v * ( 1 - s * ( 1 - f ) );
		switch( i ) {
			case 0:
				r = v;
				g = t;
				b = p;
				break;
			case 1:
				r = q;
				g = v;
				b = p;
				break;
			case 2:
				r = p;
				g = v;
				b = t;
				break;
			case 3:
				r = p;
				g = q;
				b = v;
				break;
			case 4:
				r = t;
				g = p;
				b = v;
				break;
			default:		// case 5:
				r = v;
				g = p;
				b = q;
				break;
		}
	}
}
