function R = axisangle2rot(axis, angle)
if nargin == 1
    angle = norm(axis);
    axis = axis / angle;
end
c = cos(angle); s = sin(angle); t = 1-c;
x = axis(1); y = axis(2); z = axis(3);
R = c * eye(3) + t*[x*x x*y x*z; x*y y*y y*z; x*z y*z z*z] + s*[0 -z y; z 0 -x; -y x 0];
end

