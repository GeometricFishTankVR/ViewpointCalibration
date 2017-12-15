% filename - file name of a calibration xml file from the Unity project
% P - tracker points
% Q - display calibration points
% T - Transformation matrix
function [P, Q, s, T] = getDataFromCalibrationFile(filename)

X = xml2struct(filename);
n = length(X.Calibration.displayPositions.Vector3);
m = length(X.Calibration.trackerPositions.Vector3);
assert(n == m);

P = zeros(3, n);
Q = zeros(3, n);
for i = 1:n
    p = X.Calibration.trackerPositions.Vector3{i};
    q = X.Calibration.displayPositions.Vector3{i};
    P(1, i) = str2double(p.x.Text);
    P(2, i) = str2double(p.y.Text);
    P(3, i) = str2double(p.z.Text);
    Q(1, i) = str2double(q.x.Text);
    Q(2, i) = str2double(q.y.Text);
    Q(3, i) = str2double(q.z.Text);
end

s = str2double(X.Calibration.Scale.Text);

T = zeros(4,4);
t = X.Calibration.TransformationMatrix;
T(1,1) = str2double(t.m00.Text);
T(2,1) = str2double(t.m10.Text);
T(3,1) = str2double(t.m20.Text);
T(4,1) = str2double(t.m30.Text);
T(1,2) = str2double(t.m01.Text);
T(2,2) = str2double(t.m11.Text);
T(3,2) = str2double(t.m21.Text);
T(4,2) = str2double(t.m31.Text);
T(1,3) = str2double(t.m02.Text);
T(2,3) = str2double(t.m12.Text);
T(3,3) = str2double(t.m22.Text);
T(4,3) = str2double(t.m32.Text);
T(1,4) = str2double(t.m03.Text);
T(2,4) = str2double(t.m13.Text);
T(3,4) = str2double(t.m23.Text);
T(4,4) = str2double(t.m33.Text);

