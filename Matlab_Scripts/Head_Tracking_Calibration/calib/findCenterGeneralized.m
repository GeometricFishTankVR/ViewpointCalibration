function dTD = findCenterGeneralized(hTD, vD, oV, XDC, Mi)
% This function will find the optimal center point Q, That when summed with 
% the calibration vector c rotated by R (Rotatin matrix) minimizes the 
% distance to points p.   

% M = vD*vD' - n*I;
% A = RDT*M*RDT'
% b = RDT*(vD*diag(hoD'*vD) - sum(hoD, 2));
% dT = A \ b
% dT = inv(A) * b
% inv(A) = RDT*inv(M)*RDT'; Notice that b has RDT in front, so this term 
% can be cancelled from both A and b. simplifying the entire expression to:
% dT = RDT*inv(M)*RDT'*RDT*(vD*diag(hoD'*vD) - sum(hoD, 2))
% dT = RDT*inv(M)*(vD*diag(hoD'*vD) - sum(hoD, 2))

n = size(vD, 2);
oD = zeros(3, n);

for i = 1:n
    oD(:,i) = XDC(1:3, 1:3, i)' * oV;
end

hoD = hTD - oD;

dTD = Mi * (vD * diag(hoD' * vD) - sum(hoD, 2));
end