function err = evaluateRotationAndOffset(oV, hD, XDC)
%Evaluate success of rigid transform
trans = zeros(4, size(hD, 2));
for i = 1:size(hD, 2)
    trans(:,i) = XDC(:,:,i)*[hD(1:3,i); 1] - [oV; 0];
end
displacements = sqrt(sum(trans(1:2,:).^2));
err = displacements;
end

