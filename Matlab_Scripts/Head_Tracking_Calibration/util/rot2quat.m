function q = rot2quat(R)

validateattributes(R, {'numeric'}, {'2d','real','size',[3,3]});

[axis, angle] = rot2axisangle(R);
halfangle = angle / 2;
q = [cos(halfangle); sin(halfangle) * axis];
