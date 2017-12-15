function err = evaluateRotationAndOffset3DoF(RTD, oV, hT, vD, XDC, Mi)
hTD = RTD * hT;
% find the display in tracker space
dTD = findCenterGeneralized(hTD, vD, oV, XDC, Mi);
% Construct transformation from tracker to display space
hD = bsxfun(@minus, hTD, dTD);
err = mean(evaluateRotationAndOffset(oV, hD, XDC));
end

