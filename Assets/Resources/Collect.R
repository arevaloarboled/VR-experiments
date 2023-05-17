#This code will help you to collect the data of experiments, we recommend to use `collect_spread` function
library(jsonlite)

collect<-function(results_path="./output/results/"){
	data=data.frame();
	c=0;
	# if rotation is needed
	#ang=180*pi/180;
	#rot_mat=matrix(c(cos(ang),0,sin(ang),0,1,0,-sin(ang),0,cos(ang)),3,3); #over y
	for (i in list.files(results_path, pattern="*.rds")){
	  tmp=readRDS(paste(results_path,i,sep=''));
	  tmp_data=data.frame(id=c,hearing=tmp$results$Hearing,gender=tmp$results$Gender,Age=tmp$results$Age);
	  for (j in names(tmp)[seq(2,ncol(tmp))]) {
	    temp=matrix(fromJSON(toString(tmp[j][1])),ncol = 3);  
	    # if rotation is needed
	    #temp=t(apply(temp,1,function(x) rot_mat%*%x));  
	    tmp_data[[j]]=list(temp);
	  }
	  data=rbind(data,tmp_data);
	  c=c+1;
	}
	return(data);
}

collect_spread<-function(results_path="./output/results/"){
	data=data.frame();
	ids=0;
	# if rotation is needed
	# ang=180*pi/180;
	# rot_mat=matrix(c(cos(ang),0,sin(ang),0,1,0,-sin(ang),0,cos(ang)),3,3); #over y
	for (i in list.files(results_path, pattern="*.rds")){
	  subject=readRDS(paste(results_path,i,sep=''));
	  for (j in names(subject)[seq(2,length(subject)-1)]) {
	    trajectory=matrix(fromJSON(toString(subject[j][1])),ncol = 3);  
	    # if rotation is needed
	    # trajectory=t(apply(trajectory,1,function(x) rot_mat%*%x));
	    n=nrow(trajectory);
	    data=rbind(data,data.frame(ids=ids,hearing=subject$results$Hearing,gender=subject$results$Gender,age=subject$results$Age,trajectory=j,idx=seq(0,n-1),X=trajectory[,1],Y=trajectory[,2],Z=trajectory[,3]));
	  }
	  ids=ids+1;
	}
	return(data);
}