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
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy) * 0.398942280401433;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(1) * step * Direction)) * 0.241970724519143;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(1) * step * Direction)) * 0.241970724519143;
	
	vec4 blur = avgValue / 0.882883729439719;
	vec4 outBlurred = blur * vColor.a;
	vec4 outColored = vColor * blur.a;
	
	OUTPUT_COLOR = mix(outBlurred, outColored, Mix);
}
