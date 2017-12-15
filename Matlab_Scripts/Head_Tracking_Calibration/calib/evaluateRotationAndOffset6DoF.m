function err = evaluateRotationAndOffset6DoF(RHV, oV, hT, vD, XDCn, Mi, RHTn)
n = size(hT, 2);
% estimate RTD
qTDn = zeros(n, 4);
for i = 1:n
    RCD = XDCn(1:3, 1:3, i)';
    RTH = RHTn(:, :, i)';
    qTDn(i, :) = rot2quat(XDCn(1:3,1:3,i)'*RHV'*RHTn(:,:,i)');
    qTDn(i, :) = rot2quat(RCD * RHV * RTH);
end
qTD = avg_quaternion_markley(qTDn);
RTD = quat2rot(qTD);

err = evaluateRotationAndOffset3DoF(RTD, oV, hT, vD, XDCn, Mi);
end

