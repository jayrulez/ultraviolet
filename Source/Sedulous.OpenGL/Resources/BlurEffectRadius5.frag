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
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy) * 0.0797884560802865;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(1) * step * Direction)) * 0.0782085387950912;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(1) * step * Direction)) * 0.0782085387950912;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(2) * step * Direction)) * 0.0736540280606647;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(2) * step * Direction)) * 0.0736540280606647;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(3) * step * Direction)) * 0.0666449205783599;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(3) * step * Direction)) * 0.0666449205783599;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(4) * step * Direction)) * 0.0579383105522965;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(4) * step * Direction)) * 0.0579383105522965;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(5) * step * Direction)) * 0.0483941449038286;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(5) * step * Direction)) * 0.0483941449038286;
	
	vec4 blur = avgValue / 0.729468341860768;
	vec4 outBlurred = blur * vColor.a;
	vec4 outColored = vColor * blur.a;
	
	OUTPUT_COLOR = mix(outBlurred, outColored, Mix);
}
