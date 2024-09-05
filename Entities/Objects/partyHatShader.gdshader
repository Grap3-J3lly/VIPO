shader_type spatial;

varying vec3 pos;

// uniform float offset: hint_range(0.0, 1.0, 0.01) = 0.5;

void vertex(){
    pos = VERTEX;
}

vec3 hsv2rgb(vec3 _c) {
    vec4 _K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 _p = abs(fract(_c.xxx + _K.xyz) * 6.0 - _K.www);
    return _c.z * mix(_K.xxx, clamp(_p - _K.xxx, 0.0, 1.0), _c.y);
}

void fragment() {
    ALBEDO = hsv2rgb(vec3(pos.y + TIME, 1.0, 1.0));
}