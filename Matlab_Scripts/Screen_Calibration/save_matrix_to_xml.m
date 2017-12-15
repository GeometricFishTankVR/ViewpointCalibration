function save_matrix_to_xml( mat, filename )
%SAVE_MATRIX_TO_XML Summary of this function goes here
%   Detailed explanation goes here
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

