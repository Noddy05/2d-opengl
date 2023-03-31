#version 400 core

in vec2 vCoordinates;

uniform vec4 color;

out vec4 color_out;

void main()
{
    float x = vCoordinates.x - 0.5;
    float y = vCoordinates.y - 0.5;
    if(x * x + y * y > 0.25)
        discard;
    color_out = color;
}