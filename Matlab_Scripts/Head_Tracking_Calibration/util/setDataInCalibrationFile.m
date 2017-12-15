% filename - file name of a calibration xml file from the Unity project
% T - 4x4 tranformation matrix
% O - 3x1 Offset vector
function setDataInCalibrationFile(filename, T, O)

X = xml2struct(filename);

X.Calibration.TransformationMatrix.m00.Text = T(1,1);
X.Calibration.TransformationMatrix.m01.Text = T(1,2);
X.Calibration.TransformationMatrix.m02.Text = T(1,3);
X.Calibration.TransformationMatrix.m03.Text = T(1,4);
X.Calibration.TransformationMatrix.m10.Text = T(2,1);
X.Calibration.TransformationMatrix.m11.Text = T(2,2);
X.Calibration.TransformationMatrix.m12.Text = T(2,3);
X.Calibration.TransformationMatrix.m13.Text = T(2,4);
X.Calibration.TransformationMatrix.m20.Text = T(3,1);
X.Calibration.TransformationMatrix.m21.Text = T(3,2);
X.Calibration.TransformationMatrix.m22.Text = T(3,3);
X.Calibration.TransformationMatrix.m23.Text = T(3,4);
X.Calibration.TransformationMatrix.m30.Text = T(4,1);
X.Calibration.TransformationMatrix.m31.Text = T(4,2);
X.Calibration.TransformationMatrix.m32.Text = T(4,3);
X.Calibration.TransformationMatrix.m33.Text = T(4,4);

X.Calibration.Offset.x = O(1);
X.Calibration.Offset.y = O(2);
X.Calibration.Offset.z = O(3);

struct2xml(X, filename);




