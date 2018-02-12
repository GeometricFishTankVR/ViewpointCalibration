function save_matrix_to_xml( mat, filename )
%SAVE_MATRIX_TO_XML Summary of this function goes here
%   Detailed explanation goes here
mat = matlab_to_unity_matrix_fix(mat);
docNode = com.mathworks.xml.XMLUtils.createDocument('Matrix4x4');
docRootNode = docNode.getDocumentElement;
docRootNode.setAttribute('xmlns:xsd','http://www.w3.org/2001/XMLSchema');
docRootNode.setAttribute('xmlns:xsi','http://www.w3.org/2001/XMLSchema-instance');

for i=0:3
    for j = 0:3
    thisElement = docNode.createElement(['m' int2str(i) int2str(j)]); 
    thisElement.appendChild(docNode.createTextNode(sprintf('%f',mat(i+1,j+1))));
    docRootNode.appendChild(thisElement);
    end
end


xmlFileName = [filename,'.xml'];
xmlwrite(xmlFileName,docNode);
type(xmlFileName);

end

function new_mat = matlab_to_unity_matrix_fix(mat)

% Physical Dimension of the screens (im meters)
screenWidthMeters = 0.196608;
screenHeightMeters = 0.147456;

% Reflection matrix to change hadedness
handedness_fix = eye(4);
handedness_fix(1,1) = -1;

% Setup matrices to properly position unity quads based on the top left corner of the screens 
pre_translation = eye(4);
pre_translation(1,4) = - screenWidthMeters/2;
pre_translation(2,4) = - screenHeightMeters/2;
post_translation = eye(4);
post_translation(1,4) = screenWidthMeters/2;
post_translation(2,4) = screenHeightMeters/2;

% Calculate new transformation matrix
new_mat = handedness_fix * pre_translation * mat * post_translation * handedness_fix;

end

