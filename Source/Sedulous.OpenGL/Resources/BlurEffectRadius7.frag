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
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy) * 0.0569917543430618;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(1) * step * Direction)) * 0.0564131628471802;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(1) * step * Direction)) * 0.0564131628471802;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(2) * step * Direction)) * 0.0547123942777446;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(2) * step * Direction)) * 0.0547123942777446;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(3) * step * Direction)) * 0.0519909602450691;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(3) * step * Direction)) * 0.0519909602450691;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(4) * step * Direction)) * 0.0484068479652554;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(4) * step * Direction)) * 0.0484068479652554;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(5) * step * Direction)) * 0.0441593444027238;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(5) * step * Direction)) * 0.0441593444027238;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(6) * step * Direction)) * 0.039470740790643;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(6) * step * Direction)) * 0.039470740790643;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy - (float(7) * step * Direction)) * 0.0345672463598777;
	avgValue += SAMPLE_TEXTURE2D(Texture, vTextureCoordinate.xy + (float(7) * step * Direction)) * 0.0345672463598777;
	
	vec4 blur = avgValue / 0.716433148120049;
	vec4 outBlurred = blur * vColor.a;
	vec4 outColored = vColor * blur.a;
	
	OUTPUT_COLOR = mix(outBlurred, outColored, Mix);
}
