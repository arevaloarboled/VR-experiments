library(jsonlite)
data=data.frame();
c=0;
# if rotation is needed
#ang=180*pi/180;
#rot_mat=matrix(c(cos(ang),0,sin(ang),0,1,0,-sin(ang),0,cos(ang)),3,3); #over y
for (i in list.files('./results', pattern="*.rds")){
  tmp=readRDS(paste('results/',i,sep=''));
  tmp_data=data.frame(id=c,hearing=tmp$results$Hearing,sex=tmp$results$Sex,Age=tmp$results$Age);
  for (j in names(tmp)[seq(12,ncol(tmp))]) {
    temp=matrix(fromJSON(toString(tmp[j][1])),ncol = 3);  
    #temp=t(apply(temp,1,function(x) rot_mat%*%x));  
    tmp_data[[j]]=list(temp);
  }
  data=rbind(data,tmp_data);
  c=c+1;
}
save(data,file = "Results.Rdata");