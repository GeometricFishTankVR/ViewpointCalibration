% Taken from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToAngle/
function [axis, angle] = rot2axisangle(R)
angle = acos((trace(R) - 1)/2);
d = sqrt((R(3,2) - R(2,3))^2 + (R(1,3) - R(3,1))^2 + (R(2,1) - R(1,2))^2);
x = (R(3,2) - R(2,3))/d;
y = (R(1,3) - R(3,1))/d;
z = (R(2,1) - R(1,2))/d;
axis = [x;y;z];
end

