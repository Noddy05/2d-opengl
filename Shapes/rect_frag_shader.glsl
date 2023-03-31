#version 400 core

in vec2 vCoordinates;

uniform vec4 color;

out vec4 color_out;

void main()
{
    color_out = color;
}