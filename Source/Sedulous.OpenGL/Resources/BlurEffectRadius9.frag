#includeres "Sedulous.OpenGL.Resources.SharedHeader.glsl" executing

uniform sampler2D Texture;

uniform float Resolution;
uniform float Mix;
uniform vec2  Direction;

in vec4 vColor;
in vec2 vTextureCoordinate;

DECLARE_OUTPUT_COLOR

void main()
{
	// Modified from http://callumhay.blogspot.com/2010/09/gaussian-blur-shader-glsl.html
	
	float step = 1.0 / Resolution;
	
	vec4 avgValue = vec4(0.0, 0.0, 0.0, 0.0);
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy) * 0.0443269200446036;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(1) * step * Direction)) * 0.0440541398616764;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(1) * step * Direction)) * 0.0440541398616764;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(2) * step * Direction)) * 0.0432458299079718;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(2) * step * Direction)) * 0.0432458299079718;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(3) * step * Direction)) * 0.0419314697436659;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(3) * step * Direction)) * 0.0419314697436659;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(4) * step * Direction)) * 0.0401582033203049;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(4) * step * Direction)) * 0.0401582033203049;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(5) * step * Direction)) * 0.0379880326851255;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(5) * step * Direction)) * 0.0379880326851255;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(6) * step * Direction)) * 0.035494222835817;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(6) * step * Direction)) * 0.035494222835817;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(7) * step * Direction)) * 0.0327572081038753;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(7) * step * Direction)) * 0.0327572081038753;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(8) * step * Direction)) * 0.0298603179490361;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(8) * step * Direction)) * 0.0298603179490361;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(9) * step * Direction)) * 0.0268856360576827;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(9) * step * Direction)) * 0.0268856360576827;
	
	vec4 blur = avgValue / 0.709077040974915;
	vec4 outBlurred = blur * vColor.a;
	vec4 outColored = vColor * blur.a;
	
	OUTPUT_COLOR = mix(outBlurred, outColored, Mix);
}
